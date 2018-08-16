import React from 'react';
import { render } from 'react-testing-library';
import 'dom-testing-library/extend-expect';

import MailCheckMessage from './MailCheckMessage';

describe('MailCheckMessage', () => {
  let container;

  describe('when provided with no props', () => {
    beforeEach(() => {
      ({ container } = render(
        <MailCheckMessage>
          <p>security rulez</p>
        </MailCheckMessage>
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });

  describe('when provided with the markdown flag', () => {
    beforeEach(() => {
      ({ container } = render(
        <MailCheckMessage markdown>
          {`- list item\n- another list item`}
        </MailCheckMessage>
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('render the markdown as html', () =>
      expect(container.getElementsByTagName('li')).toHaveLength(2));
  });

  describe('when provided with the markdown flag and a mailto address', () => {
    beforeEach(() => {
      ({ container } = render(
        <MailCheckMessage markdown>mailto:me@ncsc.gov.uk</MailCheckMessage>
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('it should not render a hyperlink', () =>
      expect(container.getElementsByTagName('a')).toHaveLength(0));

    test('it should contain the full mailto text', () =>
      expect(container.getElementsByTagName('p')[0]).toHaveTextContent(
        'mailto:me@ncsc.gov.uk'
      ));
  });

  describe('when provided with the markdown flag and a link', () => {
    beforeEach(() => {
      ({ container } = render(
        <MailCheckMessage markdown>https://ncsc.gov.uk</MailCheckMessage>
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('it should render a hyperlink', () =>
      expect(container.getElementsByTagName('a')[0]).toHaveTextContent(
        'https://ncsc.gov.uk'
      ));
  });
});
