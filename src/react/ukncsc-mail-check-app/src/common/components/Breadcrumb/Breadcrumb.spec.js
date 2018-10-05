import React from 'react';
import { MemoryRouter } from 'react-router-dom';
import { render } from 'react-testing-library';
import Breadcrumb from './Breadcrumb';
import BreadcrumbItem from '../BreadcrumbItem';

describe('Breadcrumb', () => {
  let container;

  test('it should match the snapshot', () => {
    ({ container } = render(
      <MemoryRouter>
        <Breadcrumb>
          <BreadcrumbItem>ncsc.gov.uk</BreadcrumbItem>
        </Breadcrumb>
      </MemoryRouter>
    ));

    expect(container).toMatchSnapshot();
  });
});
