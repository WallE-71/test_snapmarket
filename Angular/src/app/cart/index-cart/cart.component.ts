import { Component, OnInit } from '@angular/core';
import { CartService } from '../../services/cart.service';
import { Router } from '@angular/router';
import { RouterOutlet } from '@angular/router';
import { PayService } from '../../services/pay.service';
import { SecurityService } from '../../services/security.service';
import { CartDto, CartItemsDto } from '../../models';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import Swal from 'sweetalert2/dist/sweetalert2.js';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  constructor(private cartService: CartService, private payService: PayService, private securityService: SecurityService, private router: Router, private formBuilder: FormBuilder) { }

  title = 'سبد خرید';
  cart: CartDto = { productCount: 0, sumAmount: 0, data: this, cartItems: [] };
  cartItems: CartItemsDto[] = [];
  errors: string[] = [];
  form: FormGroup;

  loadItem() {
    const phoneNumber = this.securityService.GetFieldFromJWT('phoneNumber')!;
    if (phoneNumber != null) {
      this.cartService.Cart(phoneNumber).subscribe((data: any) => {
        this.cart = data.data;
        this.cartItems = data.data.cartItems;
      });
    }
    else {
      this.cart = { productCount: 0, sumAmount: 0, data: this.cart, cartItems: [] };
      this.cartItems = [];
    }
  }

  AddCount(cartItemId: number) {
    this.cartService.Increase(cartItemId).subscribe();
  }

  LowCount(cartItemId: number) {
    this.cartService.Decrease(cartItemId).subscribe();
  }

  RequestPay() {
    var discountCode = this.form.value.discountCode;
    var transport = this.form.value.transport;
    this.payService.Payment(discountCode, transport).subscribe((result: any) => {
      if (result.data == null)
        this.router.navigate(['/']);
      else {
        window.location.href = `https://sandbox.zarinpal.com/pg/StartPay/${result.data}`;
        // $.ajax({
        //   contentType: 'application/json',
        //   dataType: 'json',
        //   type: "post",
        //   url: window.location.href = `https://sandbox.zarinpal.com/pg/StartPay/${result.data}`,    ?????
        // }).then(() => {
        //   debugger
        //   window.location.href = 'http://localhost:4200'
        // })
      }
      return transport;
    })
  }

  setColor(color: any) {
    debugger
    const colors = document.querySelectorAll(".colors");
    colors.forEach(function (value) { 
      // type _colors = keyof value
      // var key = Object.keys(colors).filter(function(key) {return value[key] === color})[0];
    });
    // Object.keys(colors).find(key => colors[key] === color);
    colors.forEach(color => color.setAttribute('style', 'background-color: ' + color));
  }

  ngOnInit(): void {
    this.loadItem();
    this.form = this.formBuilder.group({
      discountCode: ['', {
        validators: [Validators.required]
      }],
      transport: ['', {
        validators: [Validators.required]
      }],
    })
  }
}
