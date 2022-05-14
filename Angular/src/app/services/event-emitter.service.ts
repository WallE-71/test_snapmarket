import { Injectable, EventEmitter } from '@angular/core';   

@Injectable({
  providedIn: 'root'
})
export class EventEmitterService {

  invokeFirstComponentFunction = new EventEmitter();    
  
  constructor() { }    
    
  onFirstComponentButtonClick() {    
    this.invokeFirstComponentFunction.emit();    
  }    
}
