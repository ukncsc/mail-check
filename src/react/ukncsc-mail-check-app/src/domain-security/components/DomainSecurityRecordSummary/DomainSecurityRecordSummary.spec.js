import React from 'react';
import { render } from 'react-testing-library';
import DomainSecurityRecordSummary from './DomainSecurityRecordSummary';

describe('DomainSecurityRecordSummary', () => {
  let container;

  describe('when no records are provided', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecurityRecordSummary type="SPF" description="bleh" />
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('it should not display a record', () =>
      expect(
        container.getElementsByClassName('DomainSecurityRecord')
      ).toHaveLength(0));
  });
});
