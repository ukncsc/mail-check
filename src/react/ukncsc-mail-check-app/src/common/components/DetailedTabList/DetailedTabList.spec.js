import React from 'react';
import { render } from 'react-testing-library';
import { DetailedTabList, DetailedTab } from 'common/components';

describe('DetailedTabList', () => {
  let container;

  beforeEach(() => {
    ({ container } = render(
      <DetailedTabList>
        <DetailedTab title="testy" headline="mc" subtitle="testface" />
      </DetailedTabList>
    ));
  });

  test('it should match the snapshot', () =>
    expect(container).toMatchSnapshot());
});
