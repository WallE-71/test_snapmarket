import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { OrderComponent } from './order/order.component';
import { SellerComponent } from './seller/seller.component';
import { ProfileComponent } from './profile/profile.component';
import { MessageComponent } from './message/message.component';
import { CategoryComponent } from './category/category.component';
import { ProductComponent } from './product/product/product.component';
import { CommentComponent } from './../app/comment/comment.component';
import { DetailsProductComponent } from './product/details/details.component';
import { SendCommentComponent } from './../app/comment/send-comment/send-comment.component';
import { SubCommentsComponent } from './../app/comment/sub-comments/sub-comments.component';

const routes: Routes = [
  { path: '', pathMatch: 'full',
    children: [
      { path: '', component: ProductComponent }]
  },
  { path: 'order', component: OrderComponent },
  { path: 'seller', component: SellerComponent },
  { path: 'profile', component: ProfileComponent },
  { path: 'message', component: MessageComponent }, 
  { path: 'category/:title', component: CategoryComponent }, 
  { path: 'details/:id', component: DetailsProductComponent,
    children: [
      { path: 'comment', component: CommentComponent, 
        children: [
          { path: 'sub-comments', component: SubCommentsComponent },
          { path: 'send-comment', component: SendCommentComponent }]
      }]
  },
  {
    path: 'cart',
    loadChildren: () => import('./cart/index-cart/cart.module').then(m => m.CartModule),
    data: { showHeader: false, showFooter: false }
  },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [RouterModule],
  providers: [
    // {provide: 'BASE_URL', useFactory: getBaseUrl} 
  ]
})
export class AppRoutingModule { }
