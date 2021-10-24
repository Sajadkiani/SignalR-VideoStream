import { Component, ElementRef } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import * as signalR from "@microsoft/signalr";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styles: []
})
export class AppComponent {
  title = 'VideoStreamAngular';

  connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/streamHub")
    .build();

  private index: number = 0;
  base64Bytes = '';

  ngOnInit() {
    debugger
    this.connection.start().then(res => {
      this.GetData();
    }).catch(err => {
      console.log(err);
    });
  }
  constructor(
    private sanitizer: DomSanitizer
  ) {

  }

  GetData() {
    this.connection.stream("Counter").subscribe({
      next: (item) => {
        // // if (this.index < 20)
        // // {
        this.base64Bytes = this.base64Bytes + item;

        // // }
        // this.index++;
      },
      complete: () => {
        // var blob=atob(this.base64Bytes);
        // let url = URL.createObjectURL(blob);
        let videoTag:any = document.getElementById("videoEl");
        videoTag.setAttribute("src", "data:video/mp4;base64," + this.base64Bytes);
        console.log(this.base64Bytes);
        // videoTag.play();
        let inp=document.getElementById("elbytes");
        // inp.setAttribute("value",this.base64Bytes);
      },
      error: (err) => {
        var li = document.createElement("li");
        li.textContent = err;
        document.getElementById("messagesList").appendChild(li);
      },
    });
  }
}
