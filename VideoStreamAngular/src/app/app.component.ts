import { ViewChild } from '@angular/core';
import { AfterViewInit, Component, ElementRef, Renderer2 } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import * as signalR from "@microsoft/signalr";
var stop=true;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styles: []
})
export class AppComponent implements AfterViewInit{
  title = 'VideoStreamAngular';
  @ViewChild('videoTag') videoEl:ElementRef; 
  connection = new signalR.HubConnectionBuilder()
  .withAutomaticReconnect()
    .withUrl("http://localhost:5000/hub/eduTv")
    // .withUrl("https://main-srv-eagri.maj.ir/hub/eduTv")
    .build();
  

  videoSource = new Array<string>();
  private i: number = 0;
  base64Bytes = '';

  ngOnInit() {
    debugger
    
    this.connect();
    // // let element:any=document.getElementById('videoPlayer').addEventListener('ended', this.myHandler, false);
    // let element:any=document.getElementById('videoPlayer');
    // element.onended=function (params) {
    //   this.myHandler()
    // }

    // this.ensureVideoPlays(); // play the video automatically
  }

  connect()
  {
    // this.connection.serverTimeoutInMilliseconds=100000000;
    // this.connection.onreconnecting=function(){
    //   console.log("reconnecting ....")
    // }

    // this.connection.onreconnected=function(){
    //   console.log("reconnected !!")
    // }

    // this.connection.keepAliveIntervalInMilliseconds = 100000;
    this.connection.start().then(res => {
      this.GetVideo();
    }).catch(err => {
      console.log(err);
    });
    
  }

  constructor(
    private sanitizer: DomSanitizer,
    // private elementRef: ElementRef,
    private renderer: Renderer2
  ) {

    // elementRef.nativeElement.querySelector('videoPlayer').addEventListener('ended', this.myHandler, false);
    // this.connection.start();

  }

  public GetVideo = () => {
    this.connection.on('GetVideoStreamAsync', (data: string) => {
      debugger
      this.videoSource.push(data);
      const element:any = document.getElementById("videoPlayer");
      if(element.currentTime > 0 &&  element.ended==false) return;

      // if(this.i>0 || (stop==false)) return;

      this.myHandler(); 
    });
  }

  ngAfterViewInit() {
    // assume dynamic HTML was added before

    this.renderer.listen(this.videoEl.nativeElement, 'ended', (event) => {
      // Do something with 'event'
      this.myHandler();
   })

    // this.videoEl.nativeElement.addEventListener('ended', 
    // this.myHandler.bind(this));
  }

  // GetData() {

  //   //   this.connection.on('GetChunck', (file) => {
  //   //     this.videoSource.push(file);
  //   //     if(this.i>0) return;

  //   //     this.myHandler(); 

  //   // });

  //   this.connection.stream("GetVideoStreamAsync").subscribe({
  //     next: (item) => {
  //       debugger
  //       this.videoSource.push(item);
  //       const element: any = document.getElementById("videoPlayer");
  //       if (element.currentTime > 0 && element.ended == false) return;

  //       // if(this.i>0 || (stop==false)) return;

  //       this.myHandler();
  //       // // if (this.index < 20)
  //       // // {
  //       // this.base64Bytes = this.base64Bytes + item;
  //       // let videoTag: any = document.getElementById("videoEl");
  //       // let source = document.createElement("source");
  //       // source.setAttribute("type", "video/mp4");
  //       // source.setAttribute("src", "data:video/mp4;base64," + this.base64Bytes);
  //       // videoTag.appendChild(source);
  //       // videoTag.load();
  //       // videoTag.play();
  //       // // }
  //       // this.index++;
  //     },
  //     complete: () => {
  //       // var blob=atob(this.base64Bytes);
  //       // let url = URL.createObjectURL(blob);
  //       // let videoTag = document.getElementById("videoEl");

  //       // let source = document.createElement("source");
  //       // source.setAttribute("type", "video/mp4");
  //       // source.setAttribute("src", "data:video/mp4;base64," + this.base64Bytes);
  //       // videoTag.appendChild(source);
  //       // // videoTag.setAttribute("src", "data:video/mp4;base64," + this.base64Bytes);
  //       // console.log(this.base64Bytes);
  //       // videoTag.play();
  //       // let inp=document.getElementById("elbytes");
  //       // inp.setAttribute("value",this.base64Bytes);
  //     },
  //     error: (err) => {
  //       debugger
  //       // this.connect();
  //     },
  //   });
  // }


// videoSource[0] = 'http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerBlazes.mp4';
// videoSource[1] = 'http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerEscapes.mp4';
// let i = 0; // global
// const videoCount = videoSource.length;

videoPlay(videoNum) {
  const element:any = document.getElementById("videoPlayer");
  element.setAttribute("src","data:video/mp4;base64,"+this.videoSource[videoNum]);
  element.autoplay = true;
  element.load();

  stop = true;
  this.videoSource[videoNum]="";
}

myHandler() {
  if (this.i == 0) {
    // this.i = 0;
    this.videoPlay(this.i);
  } else {
    this.videoPlay(this.i);
  }
  this.i++;
  stop = false;
}

ensureVideoPlays() {
  const video:any = document.getElementById('videoPlayer');

  if (!video) return;

  const promise = video.play();
  if (promise !== undefined) {
    promise.then(() => {
      // Autoplay started
    }).catch(error => {
      // Autoplay was prevented.
      video.muted = true;
      video.play();
    });
  }
}
}
