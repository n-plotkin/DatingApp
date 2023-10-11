import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})

export class RegisterComponent {
  //this is passed to us in the home component html template, which has access to it.
  @Input() usersFromHomeComponent: any;

  @Output() cancelRegister = new EventEmitter();

  constructor(private accountService: AccountService){

  }
  model: any = {}

  register(){ 
    this.accountService.register(this.model).subscribe({
      //the request only actually happens when we subscribe to it
      next: () => {
        this.cancel();
      },
      error: error => console.log(error)
    })
  }

  cancel(){
    //this will be outputted to home component html
    this.cancelRegister.emit(false);
  }
}
