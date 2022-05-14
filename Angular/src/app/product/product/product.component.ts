import { Component, Output, OnInit, EventEmitter } from '@angular/core';
import { ProductService } from '../../services/product.service';
// import { OwlOptions, CarouselModule } from 'ngx-owl-carousel-o';
import { ProductDTO } from '../../models';
import { Router } from '@angular/router';
import { ShoppingCartComponent } from '../../cart/shoppingCart/shoppingCart.component';
import { CartService } from '../../services/cart.service';
import { SecurityService } from '../../services/security.service';
import { SliderService } from '../../services/slider.service';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css']
})
export class ProductComponent implements OnInit {
  constructor(private productService: ProductService, private cartService: CartService, private securityService: SecurityService, private router: Router, private sliderService: SliderService) {}

  product1: any = [];
  product2: any = [];
  product3: any = [];
  product4: any = [];
  product5: any = [];
  product6: any = [];
  product7: any = [];
  product8: any = [];
  product9: any = [];
  product10: any = [];
  product11: any = [];
  product12: any = [];
  product13: any = [];
  product14: any = [];
  product15: any = [];

  mostTalkProduct: any;
  mostViewedProduct: any;
  mostPopularProducts: any;

  sliders: any = [];

  LoadProducts() {
    this.productService.GetProductInCategory("لبنیات").subscribe((data:any) => { this.product1 = data.data; });
    this.productService.GetProductInCategory("خواربار و نان").subscribe((data:any) => { this.product2 = data.data; });
    this.productService.GetProductInCategory("دستمال و شوینده").subscribe((data:any) => { this.product3 = data.data; });
    this.productService.GetProductInCategory("تنقلات").subscribe((data:any) => { this.product4 = data.data; });
    this.productService.GetProductInCategory("نوشیدنی").subscribe((data:any) => { this.product5 = data.data; });
    this.productService.GetProductInCategory("مواد پروتئینی").subscribe((data:any) => { this.product6 = data.data; });
    this.productService.GetProductInCategory("آرایشی و بهداشتی").subscribe((data:any) => { this.product7 = data.data; });
    this.productService.GetProductInCategory("چاشنی و افزودنی").subscribe((data:any) => { this.product8 = data.data; });
    this.productService.GetProductInCategory("میوه و سبزیجات تازه").subscribe((data:any) => { this.product9 = data.data; });
    this.productService.GetProductInCategory("کنسرو و غذای آماده").subscribe((data:any) => { this.product10 = data.data; });
    this.productService.GetProductInCategory("صبحانه").subscribe((data:any) => { this.product11 = data.data; });
    this.productService.GetProductInCategory("خشکبار، دسر و شیرینی").subscribe((data:any) => { this.product12 = data.data; });
    this.productService.GetProductInCategory("خانه و سبک زندگی").subscribe((data:any) => { this.product13 = data.data; });
    this.productService.GetProductInCategory("کودک و نوزاد").subscribe((data:any) => { this.product14 = data.data; });
    this.productService.GetProductInCategory("مد و پوشاک").subscribe((data:any) => { this.product15 = data.data; });
  }

  Index(){
    this.productService.Index().subscribe((data:any) => { 
      this.mostTalkProduct = data.data.mostTalkProduct; 
      this.mostViewedProduct = data.data.mostViewedProduct; 
      this.mostPopularProducts = data.data.mostPopularProducts; 
    });
  }

  AddToCart(productId: string){
    var shoppingCart = new ShoppingCartComponent(this.cartService, this.securityService);
    shoppingCart.SaveItem(productId);
  }
  
  Details(id: string){
    this.router.navigate(['/details', id]);
  }

  GetSliders(){
    this.sliderService.GetSliders().subscribe((data:any) => { 
      // this.sliders = data.data; 
    });
  }

  ngOnInit(): void {
    this.GetSliders();
    this.LoadProducts();
    this.Index();
  }
}
