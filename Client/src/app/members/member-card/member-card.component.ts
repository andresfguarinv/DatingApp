import { Component, computed, inject, input } from '@angular/core';
import { Member } from '../../models/member';
import { RouterLink } from '@angular/router';
import { LikesService } from '../../services/likes.service';
import { PresenceService } from '../../services/presence.service';

@Component({
  selector: 'app-member-card',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css'
})
export class MemberCardComponent {
  private likeService = inject(LikesService);
  private presenceService = inject(PresenceService);
  member = input.required<Member>();
  hasLiked = computed(() => this.likeService.likeIds().includes(this.member().id));
  isOnline = computed(() => this.presenceService.onlineUsers().includes(this.member().userName));

  toogleLike() {
    this.likeService.toogleLike(this.member().id).subscribe({
      next: () => {
        if (this.hasLiked()) {
          this.likeService.likeIds.update(ids => ids.filter(x => x !== this.member().id ))
        } else {
          this.likeService.likeIds.update(ids => [...ids, this.member().id]);
        }
      }
    });
  }
}
