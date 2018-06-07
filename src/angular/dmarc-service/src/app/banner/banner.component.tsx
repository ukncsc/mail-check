import { Component, AfterViewChecked, ViewEncapsulation } from '@angular/core';
import * as React from 'react';
import { render } from 'react-dom';

import { Provider } from 'react-redux';
import store from '../../react/store';
import { Header } from '../../react/containers';

export class BannerComponent {
  static annotations = [];

  ngAfterViewChecked() {
    render(
      <Provider store={store}>
        <Header />
      </Provider>,
      document.getElementById('header')
    );
  }
}

BannerComponent.annotations = [
  new Component({ selector: 'banner', template: '<div id="header"></div>' }),
];
