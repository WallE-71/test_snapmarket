import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

// import { OwlModule   } from 'ngx-owl-carousel';
// import { CookieService } from 'ngx-cookie-service';
// import { CarouselModule  } from 'ngx-owl-carousel-o';
import { AppRoutingModule } from './app-routing.module';
import { MaterialModule } from './material/material.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms'
// import { EventEmitterService } from './services/event-emitter.service';
import { JwtInterceptorService } from './services/jwt-interceptor.service';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { OrderComponent } from './order/order.component';
import { RatingComponent } from './rating/rating.component';
import { SellerComponent } from './seller/seller.component';
import { ProfileComponent } from './profile/profile.component';
import { UserComponent } from './security/user/user.component';
import { MessageComponent } from './message/message.component';
import { CommentComponent } from './comment/comment.component';
import { HeaderComponent } from './home/header/header.component';
import { FooterComponent } from './home/footer/footer.component';
import { CategoryComponent } from './category/category.component';
import { CreditComponent } from './security/credit/credit.component';
import { PayWaysComponent } from './cart/pay-ways/pay-ways.component';
import { ProductComponent } from './product/product/product.component';
import { DetailsProductComponent } from './product/details/details.component';
import { ShoppingCartComponent } from './cart/shoppingCart/shoppingCart.component';
import { SendCommentComponent } from './comment/send-comment/send-comment.component';
import { SubCommentsComponent } from './comment/sub-comments/sub-comments.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    UserComponent,
    OrderComponent,
    HeaderComponent,
    FooterComponent,
    RatingComponent,
    SellerComponent,
    CreditComponent,
    ProfileComponent,
    ProductComponent,
    MessageComponent,
    CommentComponent,
    PayWaysComponent,
    CategoryComponent,
    SendCommentComponent,
    SubCommentsComponent,
    ShoppingCartComponent,
    DetailsProductComponent,
  ],
  imports: [
    // OwlModule,
    FormsModule,
    BrowserModule,
    // CarouselModule,
    MaterialModule,
    HttpClientModule,
    AppRoutingModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    RouterModule.forRoot([]),
    SweetAlert2Module.forRoot()
  ],
  providers: [{
    provide: HTTP_INTERCEPTORS,
    useClass: JwtInterceptorService,
    multi: true
  },
  // CookieService
],
  bootstrap: [AppComponent]
})
export class AppModule { }
