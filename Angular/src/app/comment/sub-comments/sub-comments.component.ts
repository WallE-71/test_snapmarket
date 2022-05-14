import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import Swal from 'sweetalert2/dist/sweetalert2.js';
import { CommentService } from '../../services/comment.service';
import { SecurityService } from '../../services/security.service';

declare var $: any;

@Component({
  selector: 'app-sub-comments',
  templateUrl: './sub-comments.component.html',
  styleUrls: ['./sub-comments.component.css']
})
export class SubCommentsComponent implements OnInit {
  constructor(private commentService: CommentService, private securityService: SecurityService, private router: Router) { }

  @Input('subComments') subComments: any; 

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

  LikeOrDisLikeOfSubComments(commentId: number, isLiked: any) {
    var browserId = localStorage.getItem('browserId')!;
    if(browserId != null && browserId != 'undefined'){
      this.commentService.LikeOrDisLike(commentId, browserId, isLiked).subscribe((result:any)=> {
        $("#subCommentslike+"+commentId).html(result.data.like);
        $("#subCommentsdislike+"+commentId).html(result.data.dislike);
      });
    }
  }

  ngOnInit(): void {
  }
}
