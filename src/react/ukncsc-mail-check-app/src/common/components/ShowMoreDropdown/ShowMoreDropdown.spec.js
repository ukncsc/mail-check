import React from 'react';
import { render } from 'react-testing-library';
import ShowMoreDropdown from './ShowMoreDropdown';

describe('ShowMoreDropdown', () => {
  let container;

  beforeEach(() => {
    ({ container } = render(
      <ShowMoreDropdown>
        <p>some extra content!</p>
      </ShowMoreDropdown>
    ));
  });

  test('it should match the snapshot', () =>
    expect(container).toMatchSnapshot());
});
