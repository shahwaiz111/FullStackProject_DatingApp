import { Component, Input, input, signal } from '@angular/core';
import { Register } from "../accounts/register/register";
import { User } from '../../types/user';

@Component({
  selector: 'app-home',
  imports: [Register],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  // parent to child communication (Decorator way)
  // @Input({required:true}) membersFromApp:User[] = [];
  protected registerMode = signal(false);

  showRegister(value:boolean){
    this.registerMode.set(value);
  }
}
