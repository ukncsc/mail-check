import React from 'react';
import { render } from 'react-testing-library';
import 'dom-testing-library/extend-expect';

import DomainSecurityMessage from './DomainSecurityMessage';

describe('DomainSecurityMessage', () => {
  let container;

  describe('when provided with no props', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecurityMessage>
          <p>security rulez</p>
        </DomainSecurityMessage>
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });

  describe('when provided with the markdown flag', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecurityMessage markdown>
          {`- list item\n- another list item`}
        </DomainSecurityMessage>
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
        <DomainSecurityMessage markdown>
          mailto:me@ncsc.gov.uk
        </DomainSecurityMessage>
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
        <DomainSecurityMessage markdown>
          https://ncsc.gov.uk
        </DomainSecurityMessage>
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
