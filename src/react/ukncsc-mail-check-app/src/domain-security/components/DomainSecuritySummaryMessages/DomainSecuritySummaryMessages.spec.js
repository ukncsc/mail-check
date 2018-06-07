import React from 'react';
import { render } from 'react-testing-library';
import DomainSecuritySummaryMessages from './DomainSecuritySummaryMessages';

describe('DomainSecuritySummaryMessages', () => {
  let container;

  describe('when failures or warnings are present', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecuritySummaryMessages
          type="DMARC"
          title="bleh"
          failures={1}
          warnings={2}
        />
      ));
    });

    it('should match the snapshot', () => expect(container).toMatchSnapshot());

    it('should render the error mesage', () =>
      expect(container.getElementsByClassName('ui error message')).toHaveLength(
        1
      ));

    it('should render the warning message', () =>
      expect(
        container.getElementsByClassName('ui warning message')
      ).toHaveLength(1));

    it('should not render a success message', () =>
      expect(
        container.getElementsByClassName('ui success message')
      ).toHaveLength(0));
  });

  describe('when no failures or warnings are present', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecuritySummaryMessages
          type="SPF"
          title="bleh"
          failures={0}
          warnings={0}
        />
      ));
    });

    it('should render a success message', () =>
      expect(
        container.getElementsByClassName('ui success message')
      ).toHaveLength(1));
  });
});
