import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import { ProductService } from '../services/product.service';
import { CategoryService } from '../services/category.services';
import { Router } from '@angular/router';
import { ShoppingCartComponent } from '../cart/shoppingCart/shoppingCart.component';
import { SecurityService } from '../services/security.service';
import { CartService } from '../services/cart.service';

@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css']
})
export class CategoryComponent implements OnInit {
  constructor(private productService: ProductService, private categoryService: CategoryService,private cartService: CartService, private securityService: SecurityService, private route : ActivatedRoute, private router: Router) { }

  products:any=[];
  subCategories:any=[];

  GetProductInCategory(categoryName:string){
    this.productService.GetProductInCategory(categoryName).subscribe((data:any) => { this.products = data.data; });
  }

  LoadSubCategories(parentName:string) {
      this.categoryService.SubCategories(parentName).subscribe((data:any) => { 
      this.subCategories = data.data; 
    });
  }

  Details(id: string){
    this.router.navigate(['/details', id]);
  }

  
  AddToCart(productId: string){
    var shoppingCart = new ShoppingCartComponent(this.cartService, this.securityService);
    shoppingCart.SaveItem(productId);
  }

  ngOnInit(): void {
    var parentName = this.route.snapshot.paramMap.get('title')!;
    this.LoadSubCategories(parentName);
  }
}
