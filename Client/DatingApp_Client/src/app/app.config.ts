import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, withViewTransitions } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { errorInterceptor } from '../core/interceptors/error-interceptor';
import { jwtInterceptor } from '../core/interceptors/jwt-interceptor';
import { loadingInterceptor } from '../core/interceptors/loading-interceptor';
import { InitService } from '../core/services/init-service';
import { provideAnimations } from '@angular/platform-browser/animations';
import {provideToastr} from 'ngx-toastr';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withViewTransitions()),
    provideHttpClient(withInterceptors([errorInterceptor, jwtInterceptor, loadingInterceptor])),
    provideAppInitializer(()=> inject(InitService).init()),
    provideAnimations(),
    provideToastr({
      timeOut:5000,
      positionClass:'toast-bottom-right',
      preventDuplicates:true
      
    })
    
  ]
};
