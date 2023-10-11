/*
Component: A decorator provided by @angular/core. It's used to define a component.

OnInit: Am Angular lifecycle hook interface. Components can implement this interface to execute code when the component is initialized.
*/

import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/user';
import { Observable, of } from 'rxjs';

@Component({
  //selector: It defines the custom HTML element (tag) that represents this component 
  //when it's used in other templates. In this case, <app-nav></app-nav> will render this component.
  selector: 'app-nav',
  //templateUrl: It specifies the path to the HTML template file for this component. 
  //The template defines the structure and layout of the component's UI.
  templateUrl: './nav.component.html',
  //styleUrls: It specifies an array of CSS files to apply styles to this component. 
  //Styles defined here are scoped to the component, ensuring encapsulation.
  styleUrls: ['./nav.component.css']
})

//This declares the NavComponent class, which represents the Angular component.//
//""export class"": Indicates that this class can be imported and used in other parts of the Angular project.
//implements OnInit means the component will have a ngOnInit method called on initializaiton
export class NavComponent implements OnInit{

  //class property to contain form data, empty object
  model: any = {};

  //dependency injection / services would go here in the constructor
  constructor(public accountService: AccountService) { }

  ngOnInit(): void {
  }

  login() {
    //subscribe to the observable
    this.accountService.login(this.model).subscribe({
        next: response => {
          console.log(response);
        },
        error: error => console.log(error)
    })
  }
  
  logout(){
    this.accountService.logout();
  }

}
