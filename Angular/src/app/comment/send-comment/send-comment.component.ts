import { Component, OnInit, Input } from '@angular/core';
import { CommentService } from '../../services/comment.service';
import { ActivatedRoute } from "@angular/router";
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import Swal from 'sweetalert2/dist/sweetalert2.js';
import { SecurityService } from '../../services/security.service';

declare var $: any;

@Component({
  selector: 'app-send-comment',
  templateUrl: './send-comment.component.html',
  styleUrls: ['./send-comment.component.css']
})
export class SendCommentComponent implements OnInit {
  constructor(private commentService: CommentService, private securityService: SecurityService, private formBuilder: FormBuilder, private route: ActivatedRoute) { }

  form: FormGroup;
  productId: any;
  parentCommentId: any;
  // @Input() comment: any;

  SendComment() {
    debugger
    var browserId = localStorage.getItem('browserId')!;
    if(browserId != null && browserId != 'undefined'){
      var name = this.form.value.name;
      var email = this.form.value.email;
      var description = this.form.value.description;
      var parentCommentId = this.parentCommentId;

      this.commentService.SendComment(name, email, description, this.productId[1], parentCommentId).subscribe((result:any)=>{
        if(result.isSuccess){
          return Swal.fire({
            icon: 'success',
            title: result.data,
            confirmButtonText: 'بستن',
          })
        }
        return result.data;
      });
    }
    else{
      return Swal.fire({
        icon: 'error',
        title: 'ابتدا وارد شوید!',
        confirmButtonText: 'بستن',
      })
    }
    return null;
    // var form = $("#reply-" + this.parentCommentId).find('form');
    // var actionUrl = form.attr('action');

    // var loaderAfter = "#comment-" + parentCommentId;
    // if ($("#comment-" + parentCommentId).length == 0) {
    //   loaderAfter = "#reply-"
    // }
    // $.ajax({
    //   url: this.commentService.SendComment(name, email, description, this.productId[1], parentCommentId).subscribe(),
    //   beforeSend: function () {
    //     $(".vizew-btn").attr("disabled", true);
    //     $(loaderAfter).after("<p class='text-center mb-5 mt-3'><span style='font-size:18px;font-family: Vazir_Medium;'> در حال ارسال دیدگاه  </span><img src='/icons/LoaderIcon.gif'/></p>")
    //   },
    //   complete: function () {
    //     $(".vizew-btn").attr("disabled", false);
    //     $(loaderAfter).next().replaceWith("");
    //   }
    // }).done(function (data: any) {
    //   debugger
    //   var newForm = $("form", data);
    //   $("#reply-" + parentCommentId).find("form").replaceWith(newForm);
    //   var IsValid = newForm.find("input[name='IsValid']").val() === "True";
    //   if (IsValid) {
    //     $("#comment-" + parentCommentId).next().replaceWith("");
    //     $("#comment-" + parentCommentId).next().replaceWith("");
    //     $.ajax({
    //       url: '/Admin/Base/Notification', error: function () {
    //         Swal.fire({
    //           title: 'خطایی رخ داده است !!!',
    //           text: 'لطفا تا برطرف شدن خطا شکیبا باشید.',
    //           confirmButtonText: 'بستن'
    //         });
    //       }
    //     }).done(function () {
    //       Swal.fire({
    //         title: 'عملیات با موفقیت انجام شد',
    //         confirmButtonText: 'بستن',
    //       })
    //     });
    //     $("#Name").val("");
    //     $("#Email").val("");
    //     $("#Desription").val("");
    //   }
    // });
  }

  ngOnInit(): void {

    this.form = this.formBuilder.group({
      name: ['', {
        validators: [Validators.required]
      }],
      email: ['', {
        validators: [Validators.required]
      }],
      description: ['', {
        validators: [Validators.required]
      }]
    })

    var productId = this.route.snapshot.paramMap.get('productId')!;
    if (productId != null)
      this.productId = productId;
    else
      this.productId = window.location.href.split('http://localhost:4200/details/')!;
    var parentCommentId = this.route.snapshot.paramMap.get('parentCommentId')!;
    if (parentCommentId != null)
      this.parentCommentId = parentCommentId;
    else
      this.parentCommentId = null;
  }
}
