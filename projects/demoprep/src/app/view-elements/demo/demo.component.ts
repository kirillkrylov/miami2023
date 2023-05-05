import {Component, HostBinding, Input, OnInit} from "@angular/core";
import {CrtInput, CrtInterfaceDesignerItem, CrtViewElement, HttpClientService} from "@creatio-devkit/common";

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

export class DemoComponent  implements OnInit{
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
		this._userEmail = v;
		(async ()=>{
			const login = await this.getData(this.userEmail);
			await this.getCalendar(login);
		})();
	}
	

	ngOnInit(): void {
		
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
		const httpClient = new HttpClientService();
		const result = await httpClient.get(
			`/rest/GitHubData/FindUser?searchValue=${userEmail}`,
			{responseType: "text"});

		if(result.body){
			const login = JSON.parse(result.body).value[0].Login
			return login;
		}
		return "";
	}

	async getCalendar(login:string):Promise<void>{
		const httpClient = new HttpClientService();
		const result = await httpClient.get(
			`/rest/GitHubData/GetCalendarForUser?login=${login}`,
			{responseType: "text"});
		if(result.body){
			const daysArray:Array<number> =  JSON.parse(result.body)['value']
			daysArray.forEach(d=>{
				this.calendarData.push(`c${d}`); 
			})
			console.log(result.body);
		}
	}
}