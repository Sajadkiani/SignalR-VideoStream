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

  ngOnInit() {
    debugger
    this.connection.start().then(res => {
      this.GetData();
    }).catch(err => {
      console.log(err);
    });
  }

  GetData() {
    this.connection.stream("Counter", 100, 500).subscribe({
      next: (item) => {
        var span = document.createElement("li");
        span.style.border="1px solid lightgray";
        span.textContent = item;
        document.getElementById("messagesList").appendChild(span);
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
