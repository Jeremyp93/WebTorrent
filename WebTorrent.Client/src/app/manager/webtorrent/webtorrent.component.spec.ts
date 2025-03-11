import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WebtorrentComponent } from './webtorrent.component';

describe('WebtorrentComponent', () => {
  let component: WebtorrentComponent;
  let fixture: ComponentFixture<WebtorrentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WebtorrentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WebtorrentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
