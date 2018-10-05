import React from 'react';
import { render } from 'react-testing-library';
import 'dom-testing-library/extend-expect';
import DomainSecurityRecord from './DomainSecurityRecord';

describe('DomainSecurityRecord', () => {
  let container;

  describe('when the record is not inherited', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecurityRecord>record1</DomainSecurityRecord>
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });

  describe('when the record is inherited', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecurityRecord type="DMARC" inheritedFrom='foo.com'>
          record2
        </DomainSecurityRecord>
      ));
    });

    test('it should say so', () =>
      expect(container.querySelector('h3')).toHaveTextContent(
        'Record inherited from foo.com'
      ));
  });
});
