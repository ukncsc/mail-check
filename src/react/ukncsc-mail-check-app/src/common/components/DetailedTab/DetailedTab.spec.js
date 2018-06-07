import React from 'react';
import { render } from 'react-testing-library';
import DetailedTab from './DetailedTab';

describe('DetailedTab', () => {
  let container;

  describe('when the tab is the first one', () => {
    let tabSelectedMock;

    beforeEach(() => {
      tabSelectedMock = jest.fn();

      ({ container } = render(
        <DetailedTab
          title="My first tab"
          headline="101"
          subtitle="cool"
          index={0}
          activeIndex={0}
          tabSelected={tabSelectedMock}
        >
          <p>Hello World</p>
        </DetailedTab>
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('should should ensure that a tab is selected', () =>
      expect(tabSelectedMock).toHaveBeenCalled());
  });
});
