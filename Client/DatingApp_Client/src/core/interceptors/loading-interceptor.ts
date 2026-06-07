import { HttpInterceptorFn, HttpParams } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../services/busy-service';
import { delay, finalize, of, tap } from 'rxjs';

const cache = new Map<string, any>();

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);

  // bcz now we've got a way to store the query string plus the URL. so when we make the same request to the API that we've
  // already gone to before, so long as we havent refresh our page, then we'll be able to get the response from cache instead
  // of going out to our API everytime..
  const generateCacheKey = (url:string, params:HttpParams):string =>{
    const paramString = params.keys().map(key=>`${key}=${params.get(key)}`).join('&');
    return paramString? `${url}?${paramString}`: url;
  }

  const invalidateCache = (urlPattern:string) =>{
    for (const key of cache.keys()){
      if (key.includes(urlPattern)){
        cache.delete(key);
        console.log(`cache invalidated: ${key}`);
      }
    }
  }

  const cacheKey = generateCacheKey(req.url, req.params);

  if (req.method.includes('POST') && req.url.includes('/likes')){
    invalidateCache('/likes')
  }
  

  if (req.method === 'GET'){
    const cachedResponse = cache.get(cacheKey);
    if (cachedResponse){
      return of(cachedResponse);
    }
  }

  busyService.busy();
  return next(req).pipe(
    delay(500),
    tap(response=>{
      cache.set(cacheKey, response)
    }),
    finalize(()=>{
      busyService.idle()
    })
  )
};
