import React from 'react';
import { MemoryRouter } from 'react-router-dom';
import { render } from 'react-testing-library';
import 'dom-testing-library/extend-expect';

import DomainSecuritySummaryTxt from './DomainSecuritySummaryTxt';

const fetchDomainSecurityTxt = jest.fn();
const getDomainSecurityTxt = jest.fn();

describe('DomainSecuritySummaryTxt', () => {
  let container;

  afterEach(() => jest.resetAllMocks());

  describe('when no records are provided', () => {
    beforeEach(() => {
      ({ container } = render(
        <MemoryRouter>
          <DomainSecuritySummaryTxt
            domainId={123}
            fetchDomainSecurityTxt={fetchDomainSecurityTxt}
            getDomainSecurityTxt={getDomainSecurityTxt}
            type="SPF"
          />
        </MemoryRouter>
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
