import { Component } from '@angular/core';
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
  base64Bytes:string=""; 

  ngOnInit() {
    debugger
    this.connection.start().then(res => {
      this.GetData();
    }).catch(err => {
      console.log(err);
    });
  }

  GetData() {
    this.connection.stream("Counter").subscribe({
      next: (item) => {
        // if (this.index < 30)
        //   this.base64Bytes += item;
      },
      complete: () => {
        var li = document.createElement("li");
        li.textContent = "Stream completed";
        document.getElementById("messagesList").appendChild(li);
      },
      error: (err) => {
        var li = document.createElement("li");
        li.textContent = err;
        document.getElementById("messagesList").appendChild(li);
      },
    });
  }  
}
