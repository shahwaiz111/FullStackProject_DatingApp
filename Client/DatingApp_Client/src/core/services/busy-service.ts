import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class BusyService {
  busyRequestCount = signal(0);

  busy(){
    this.busyRequestCount.update(current=> current + 1);
  }

  idle(){ // To reduce the busyCount by 1..
    this.busyRequestCount.update(current=> Math.max(0, current - 1));
  }
}
