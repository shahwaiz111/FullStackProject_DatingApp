import { Component, inject, input, Input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RegisterCreds, User } from '../../../types/user';
import { AccountService } from '../../../core/services/account-service';


@Component({
  selector: 'app-register',
  imports: [FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {

  private accountService = inject(AccountService);
  // parent to child communication (Signal way)
  // public membersFromHome = input.required<User[]>();

  // // child to parent communication (Signal way)
  cancelRegister = output<boolean>();
  protected creds = {} as RegisterCreds;

  register(){
     this.accountService.register(this.creds).subscribe({
      next:response=> {
        console.log(response);
        this.cancel();
      },
      error: error=> console.log(error)
    })
  }

  cancel(){
    // console.log('cancelled!');
    this.cancelRegister.emit(false)
  }
}
