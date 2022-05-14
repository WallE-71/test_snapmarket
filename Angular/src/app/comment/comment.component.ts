import { Component, OnInit } from '@angular/core';
import { ProductService } from '../services/product.service';
import { CommentService } from '../services/comment.service';
import { Router } from '@angular/router';
import Swal from 'sweetalert2/dist/sweetalert2.js';
import { SecurityService } from '../services/security.service';

declare var $: any;

@Component({
  selector: 'app-comment',
  templateUrl: './comment.component.html',
  styleUrls: ['./comment.component.css']
})
export class CommentComponent implements OnInit {
  constructor(private productService: ProductService, private commentService: CommentService, private securityService: SecurityService, private router: Router) { }

  productComments: any = [];

  Comments(productId: string) {
    this.productService.ProductDetails(productId).subscribe((result: any) => {
      if (result.data.productComments != null)
        this.productComments = result.data.productComments;
    });
  }

  ShowCommentForm(parentCommentId: string, productId: string) {
    debugger
    $.ajax({
      url: this.router.navigate(['/send-comment', parentCommentId, productId]),
      beforeSend: function () { $("#comment-" + parentCommentId).after("<p class='text-center mb-5 mt-3'><span style='font-size:18px;font-family: Vazir_Medium;'> لطفا منتظر بماند  </span><img src='/icons/LoaderIcon.gif'/></p>") },
      error: function () {
        Swal.fire({
          title: 'خطایی رخ داده است !!!',
          text: 'لطفا تا برطرف شدن خطا شکیبا باشید.',
          confirmButtonText: 'بستن'
        });
      }
    }).done(function (result: any) {
      $("#comment-" + parentCommentId).next().replaceWith("");
      $("#comment-" + parentCommentId).after("<hr/>" + result);
      $("#btn-" + parentCommentId).html("لغو پاسخ");
      $("#btn-" + parentCommentId).attr("onclick", "HideCommentForm('" + parentCommentId + "','" + productId + "')");
    });
  }

  HideCommentForm(parentCommentId: string, productId: string) {
    debugger
    $("#comment-" + parentCommentId).next().replaceWith("");
    $("#comment-" + parentCommentId).next().replaceWith("");
    $("#btn-" + parentCommentId).html("پاسخ");
    $("#btn-" + parentCommentId).attr("onclick", "ShowCommentForm('" + parentCommentId + "','" + productId + "')");
  }

  LikeOrDisLike(commentId: number, isLiked: boolean) {
    var browserId = localStorage.getItem('browserId')!;
    if(browserId != null && browserId != 'undefined'){
      this.commentService.LikeOrDisLike(commentId, browserId, isLiked).subscribe((result:any)=> {
        debugger
        $("#commentlike+"+commentId).html(result.data.like);
        $("#commentdislike+"+commentId).html(result.data.dislike);
      });
    }
  }

  LikeOrDisLikeOfSubComments(commentId: number, isLiked: boolean) {
    var browserId = localStorage.getItem('browserId')!;
    if(browserId != null && browserId != 'undefined'){
      this.commentService.LikeOrDisLike(commentId, browserId, isLiked).subscribe((result:any)=> {
        $("#sublike+"+commentId).html(result.data.like);
        $("#subdislike+"+commentId).html(result.data.dislike);
      });
    }
  }

  ngOnInit(): void {
    const productId = window.location.href.split('http://localhost:4200/details/')!;
    this.Comments(productId[1]);
  }
}
