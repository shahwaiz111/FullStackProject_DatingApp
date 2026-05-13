import { Component, inject, OnInit, signal } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { Member, Photo } from '../../../types/member';
import { AsyncPipe } from '@angular/common';
import { ImageUpload } from "../../../shared/image-upload/image-upload";
import { AccountService } from '../../../core/services/account-service';

@Component({
  selector: 'app-member-photos',
  imports: [AsyncPipe, ImageUpload],
  templateUrl: './member-photos.html',
  styleUrl: './member-photos.css',
})
export class MemberPhotos implements OnInit {
  protected memberService = inject(MemberService);
  private accountService = inject(AccountService);
  private route = inject(ActivatedRoute);
  // protected photos$?:Observable<Photo[]>;
  protected photos = signal<Photo[]>([]);
  protected loading = signal(false);
  
  ngOnInit(): void {
    const memberId = this.route.parent?.snapshot.paramMap.get('id');
    if (memberId){
      // this.photos$ = this.memberService.getMemberPhotos(memberId);
       this.memberService.getMemberPhotos(memberId).subscribe({
        next: photos => this.photos.set(photos)
       })
  }

}
  onUploadImage(file:File){
    this.loading.set(true);
    this.memberService.uploadPhoto(file).subscribe({
      next: photo=>{
        this.memberService.editMode.set(false);
        this.loading.set(false);
        this.photos.update(photos=> [...photos, photo])
        if (!this.memberService.member()?.imageUrl){
          this.setMainLocalPhoto(photo);
        }
      },
      error: error=>{
        console.log('Error uploading image:', error);
        this.loading.set(false);
      }
    })
  }

  setMainPhoto(photo:Photo){
    this.memberService.setMainPhoto(photo).subscribe({
      next: ()=>{
        this.setMainLocalPhoto(photo);

        // const currentUser = this.accountService.currentUser();
        // if (currentUser){
        //   currentUser.imageUrl = photo.url;
        //   this.accountService.setCurrentUser(currentUser);
        //   this.memberService.member.update(member=> ({
        //     ...member, imageUrl:photo.url 
        //   }) as Member)
        
      }
    })
  }

  deletePhoto(photoId:number){
    this.memberService.deletePhoto(photoId).subscribe({
      next: () => {
        this.photos.update(photos=> photos.filter(x=>x.id != photoId))
      }, // we dont get back anything from the APi
      error:err=> console.log(err)
    })
  }

  private setMainLocalPhoto(photo:Photo){
    const currentUser = this.accountService.currentUser();
        if (currentUser){
          currentUser.imageUrl = photo.url;
          this.accountService.setCurrentUser(currentUser);
          this.memberService.member.update(member=> ({
            ...member, imageUrl:photo.url 
          }) as Member)
  }
}}
