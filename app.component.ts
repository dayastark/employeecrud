import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { AppService } from './app.service';
import { UserViewModel } from './models/user-view-model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  user: UserViewModel = {} as UserViewModel;
  now: Date = new Date();
  currentPlantCode = "";
  plantCodes: string[] = [];
  loading = false;

  constructor(
    private appService: AppService,
    private message: NzMessageService,
    private router: Router,
  ) {
    this.loading = true;
    this.appService.getCurrentUser().subscribe(m => {

      this.user = m.info as UserViewModel;
      this.plantCodes = this.user.plantCodes;
      sessionStorage.setItem('user', JSON.stringify(this.user));
      if (sessionStorage.getItem('plantCode') === null) {
        const plantCode = this.plantCodes.length > 0 ? this.plantCodes[0] : "";
        sessionStorage.setItem('plantCode', plantCode);
      } else {
        this.currentPlantCode = sessionStorage.getItem('plantCode') as string;
      }
      this.loading = false;

    }, (e) => {
      this.loading = false;
      this.message = e.message;
    });
  }

  changePlantCode(changedValue: string) {
    this.loading = true;
    this.currentPlantCode = changedValue;
    sessionStorage.setItem('plantCode', this.currentPlantCode);
    if (this.router.url.indexOf('details') !== -1) {
      let baseUrl = this.router.url.split('/');
      alert(baseUrl);
      window.location.href = `${baseUrl[0]}/${baseUrl[1]}`;
      this.loading = false;
    }
    else {
      window.location.reload();
      this.loading = false;
    }

  }

  ngOnInit(): void {
    setInterval(() => this.now = new Date(), 1);
  }

  isCollapsed = false;
}
