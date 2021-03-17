import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { City } from './city';

@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrls: ['./city-edit.component.scss']
})
export class CityEditComponent implements OnInit {
  title: string;
  form: FormGroup;
  city: City;


  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl:string
  ) { }

  ngOnInit(): void {
    this.form = new FormGroup({
      name: new FormControl(''),
      lat: new FormControl(''),
      lon: new FormControl('')
    });

    this.loadData();
  }
    loadData() {
      var id = +this.activatedRoute.snapshot.paramMap.get('id');

      var url = this.baseUrl + 'api/Cities/' + id;
      this.http.get<City>(url).subscribe(result => {
        this.city = result;
        this.title = "Edit - " + this.city.name;

        this.form.patchValue(this.city);
      }, error => console.error(error));
  }

  onSubmit() {
    var city = this.city;

    city.name = this.form.get("name").value;
    city.lat = +this.form.get("lat").value;
    city.lon = +this.form.get("lon").value;

    var url = this.baseUrl + "api/Cities/" + this.city.id;
    this.http.put<City>(url, city)
      .subscribe(result => {
        console.log("City " + city.id + " has been updated.");
        this.router.navigate(['/Cities']);
      }, error => console.error(error));
  }

}