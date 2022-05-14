import { Component, OnInit, Input } from '@angular/core';
import { TreeViewCategoryDto } from '../../models';
import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.services';
import { Router } from '@angular/router';
import Swal from 'sweetalert2/dist/sweetalert2.js';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  constructor(private categoryService: CategoryService,private productService: ProductService, private formBuilder: FormBuilder, private router: Router) { 
    this.form = formBuilder.group({
      productName: ['', {
        validators: [Validators.required]
      }]
    })
  }

  form: FormGroup;
  @Input() productName: any;
  categories: TreeViewCategoryDto[] = [];

  Search() {
    var productName = this.form.value.productName;
    this.productService.Search(productName).subscribe((result: any) => {
      if (result.isSuccess == true)
        this.router.navigate(['/details', result.data]);
      else {
        return Swal.fire({
          title: result.data,
          confirmButtonText: 'بستن'
        });
      }
      return productName;
    });
  }

  LoadCategories() {
    this.categoryService.GetCategories().subscribe((data: any) => {
      this.categories = data.data;
    });
  }

  ngOnInit(): void {
    this.LoadCategories();
  }
}
