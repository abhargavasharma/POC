import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { DrawShapeComponent } from './draw-shape.component';
import { NgvasModule, tweens, hitAreas } from "ngvas";

describe('DrawShapeComponent', () => {
  let component: DrawShapeComponent;
  let fixture: ComponentFixture<DrawShapeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DrawShapeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DrawShapeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
