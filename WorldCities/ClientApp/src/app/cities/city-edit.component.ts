import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Country } from '../countries/country';
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
  id?: number;
  countries: Country[];


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
      lon: new FormControl(''),
      countryId: new FormControl(''),
    });

    this.loadData();
  }
  loadData() {
    this.loadCountries();

      this.id = +this.activatedRoute.snapshot.paramMap.get('id');

      if (this.id) {
        var url = this.baseUrl + 'api/Cities/' + this.id;
        this.http.get<City>(url).subscribe(result => {
          this.city = result;
          this.title = "Edit - " + this.city.name;

          this.form.patchValue(this.city);
        }, error => console.error(error));
      } else {
        this.title = "Create a new City";
      }

     
  }
    loadCountries() {
      var url = this.baseUrl + "api/Countries";
      var params = new HttpParams()
        .set('pageIndex', "0")
        .set('pageSize', "9999")
        .set('sortColumn', "name");
      this.http.get<any>(url, { params })
        .subscribe(result => {
          this.countries = result.data;
        }, error => console.error(error));
    }

  onSubmit() {
    var city = (this.id) ? this.city : <City>{};

    city.name = this.form.get("name").value;
    city.lat = +this.form.get("lat").value;
    city.lon = +this.form.get("lon").value;
    city.countryId = +this.form.get("countryId").value;

    if (this.id) {
      var url = this.baseUrl + "api/cities/" + this.city.id;
      this.http.put<City>(url, city)
        .subscribe(result => {
          console.log("City " + city.id + " has been updated.");
          this.router.navigate(['/cities']);
        }, error => console.error(error));
    } else {
      var url = this.baseUrl + "api/Cities";
      this.http.post<City>(url, city)
        .subscribe(result => {
          console.log("City " + result.id + " has been created.");

          this.router.navigate(['/cities']);
        }, error => console.error(error));
    }

   
  }

}
