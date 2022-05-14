import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { SecurityService } from '../../services/security.service';
import { RatingService } from '../../services/rating.service';
import { ActivatedRoute } from "@angular/router";
import Swal from 'sweetalert2/dist/sweetalert2.js';

declare var $: any;

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.css']
})
export class DetailsProductComponent implements OnInit {
  constructor(private productService: ProductService, private securityService: SecurityService, private ratingsService: RatingService, private route: ActivatedRoute) { }

  seller: any = [];
  product: any = [];
  productsRelated: any = [];
  productsPrefered: any = [];
  imageFiles: any = [];
  private productId: string;

  Details(productId: string) {
    this.productService.ProductDetails(productId).subscribe((result: any) => {
      if (result.data != null) {
        if (result.data.product != null)
          this.imageFiles = result.data.product.imageFiles;
        this.product = result.data.product;
        this.seller = result.data.seller;
        this.productsRelated = result.data.productRelated;
        this.productsPrefered = result.data.productsPrefered;
      }
    });
  }

  Bookmark() {
    var userId = this.securityService.GetFieldFromJWT('id')!;
    this.productService.Bookmark(this.productId, userId).subscribe((result: any) => {
      if (result.data == true) {
        $("#bookmark").html('<i aria-hidden="true" class="fa fa-bookmark"></i>');
      }
      else if (result.data == false) {
        $("#bookmark").html('<i aria-hidden="true" class="fa fa-bookmark-o"></i>');
      }
      else {
        Swal.fire({
          icon: 'error',
          title: 'امکان بوکمارک نیست !!!',
          text: 'برای بوکمارک کردن محصول ابتدا باید وارد سایت شوید.',
          confirmButtonText: 'بستن'
        });
      }
    });
  }

  Rating(sellerId: number, rate: number) {
    var userId = this.securityService.GetFieldFromJWT('id')!;
    if (userId == '') {
      Swal.fire({
        icon: 'error',
        title: 'امکان امتیاز دهی نیست !!!',
        text: 'برای امتیاز دهی به فروشنده ابتدا باید وارد سایت شوید.',
        confirmButtonText: 'بستن'
      });
    }
    else {
      this.ratingsService.Rate(sellerId, this.productId, userId, rate).subscribe((result: any) => {
        if (result == false) {
          Swal.fire({
            icon: 'error',
            title: 'امکان امتیاز دهی نیست !!!',
            text: 'برای امتیازدهی به فروشنده باید محصولی از فروشنده خرید کرده باشید.',
            confirmButtonText: 'بستن'
          });
        }
      });
    }
  }

  ngOnInit(): void {
    this.productId = this.route.snapshot.paramMap.get('id')!;
    this.Details(this.productId);
  }
}
