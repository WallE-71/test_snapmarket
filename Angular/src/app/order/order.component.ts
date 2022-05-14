import { Component, OnInit } from '@angular/core';
import { OrderService } from '../services/order.service';

declare var $: any;

@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css']
})
export class OrderComponent implements OnInit {
  constructor(private orderService: OrderService) { }

  orders: any = [];
  status: number = 0;

  ShowOrder() {
    this.orderService.Order().subscribe((data: any) => {
      this.orders = data.data;
    });
  }

  RemoveOrder(orderId: number){
    this.orderService.Remove(orderId).subscribe((result: any) => {
      if (result.data == true) {

        // $("#remove").html(' style="display:none;transition: 0.5s linear;transition-timing-function: ease-in;');
      }
    });
  }

  ngOnInit(): void {
    this.ShowOrder();
  }
}
