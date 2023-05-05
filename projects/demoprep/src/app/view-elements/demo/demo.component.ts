import {Component, HostBinding, Input, OnDestroy, OnInit} from "@angular/core";
import { FormControl, Validators } from "@angular/forms";
import {CrtInput, CrtInterfaceDesignerItem, CrtViewElement, HttpClientService} from "@creatio-devkit/common";
import { SubscriptionLike, debounceTime, filter, switchMap } from "rxjs";

@CrtViewElement({
  selector: "mrkt-demo",
  type: "mrkt.Demo",
  inputs: {
	userDefinedHue: 0,
	},
})

@CrtInterfaceDesignerItem({
  toolbarConfig: {
    caption: "Github style calendar",
    name: "mrkt-demo",
    icon: require("!!raw-loader?{esModule:false}!./github.svg"),
	hint: "Github inspired calendar"
  },
})

@Component({
  selector: "mrkt-demo",
  templateUrl: './demo.component.html',
  styleUrls : ['./demo.component.css']
})

export class DemoComponent  implements OnInit, OnDestroy {
	private _control = new FormControl('', [Validators.minLength(5)]);
	private _subscription?: SubscriptionLike;
	@HostBinding("style.--hue") cssDefaultHue: number = 130; //github style color
	calendarData = new Array<string>();

	constructor() { }
	@Input()
	public loading = false;

	private _userDefinedHue : number = 0;
	public get userDefinedHue() : number {
		return this._userDefinedHue;
	}
	@Input() 
	@CrtInput()
	public set userDefinedHue(v : number) {
		this._userDefinedHue = v;
		this.cssDefaultHue = v;
	}
	
	@Input() 
	@CrtInput()
	userDefinedData: Array<number> = new Array<number>();

	
	private _userEmail : string = "";
	public get userEmail() : string {
		return this._userEmail;
	}
	@Input() 
	@CrtInput()
	public set userEmail(v : string) {

		this._control.setValue(v);
		this._userEmail = v;
	}
	

	ngOnInit(): void {
		this._subscription = this._control.valueChanges.pipe(
			filter(() => Boolean(this._control.value)),
			filter(x => this._control.valid),
			debounceTime(1000),
			switchMap(() => this.getData(this._control.value as string)),
			filter(login => Boolean(login)),
			switchMap(login => this.getCalendar(login))
		).subscribe()
		// this.userDefinedData.forEach(d=>{
		// 	this.calendarData.push(`c${d}`);
		// });

		// for(var i = 0; i<52*7; i++){
		// 	const c = this.getRandomInt(1,5);
		// 	this.calendarData.push(`c${c}`); 
		// }
	}

	/** Calculates random integers between min and max
	 *  The maximum is exclusive and the minimum is inclusive
	 * @param min 
	 * @param max 
	 * @returns Random integer
	 */
	getRandomInt(min: number, max: number) {
		min = Math.ceil(min);
		max = Math.floor(max);
		return Math.floor(Math.random() * (max - min) + min); // The maximum is exclusive and the minimum is inclusive
	}

	/** Gets data from github
	 * 
	 */
	async getData(userEmail: string):Promise<string>{
		try {
			const httpClient = new HttpClientService();
			const result = await httpClient.get(
				`/rest/GitHubData/FindUser?searchValue=${userEmail}`,
				{responseType: "text"});

			if(result.body){
				const login = JSON.parse(result.body).value[0].Login
				return login;
			}
		} catch(_) {}
		return "";
	}

	async getCalendar(login:string):Promise<void>{
		const httpClient = new HttpClientService();
		const result = await httpClient.get(
			`/rest/GitHubData/GetCalendarForUser?login=${login}`,
			{responseType: "text"});
		if(result.body){
			const daysArray:Array<number> =  JSON.parse(result.body)['value']
			
			this.calendarData = new Array<string>();
			daysArray.forEach(d=>{
				
				this.calendarData.push(`c${d}`); 
			})
			console.log(result.body);
		}
	}

	public ngOnDestroy() {
		this._subscription?.unsubscribe();
	}
}