import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { catchError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError(error=>{
      if (error){
        switch (error.status) {
          case 400:
            if (error.error.errors){
              const modelStateErrors = [];
              for (const key in error.error.errors){
                if (error.error.errors[key]){
                  modelStateErrors.push(error.error.errors[key])
                }}
                throw modelStateErrors.flat();
            }
              else{
                alert(error.error)
              }
            
            break;
      
            case 401:
              alert("unauthorized");
              break;

            case 404:
              // alert("Not Found");
              router.navigateByUrl('/not-found')
              break;

            case 500:
            // alert("Server error");
            const navigationExtra: NavigationExtras = {state:{error: error.error}}
            router.navigateByUrl('/server-error', navigationExtra)
            break;
          default:
            alert("something went wrong..")
            break;
        }
      }
      throw error;
    })
  );
};
