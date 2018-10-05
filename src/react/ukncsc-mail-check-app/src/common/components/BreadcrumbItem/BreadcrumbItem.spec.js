import React from 'react';
import { MemoryRouter } from 'react-router-dom';
import { render } from 'react-testing-library';
import BreadcrumbItem from './BreadcrumbItem';

describe('BreadcrumbItem', () => {
  let container;

  test('it should match the snapshot', () => {
    ({ container } = render(
      <MemoryRouter>
        <BreadcrumbItem>ncsc.gov.uk</BreadcrumbItem>
      </MemoryRouter>
    ));

    expect(container).toMatchSnapshot();
  });
});
