import React from 'react';
import { MemoryRouter } from 'react-router-dom';
import { render } from 'react-testing-library';
import DomainSecurityDetailsMx from './DomainSecurityDetailsMx';

describe('DomainSecurityDetailsMx', () => {
  let container;

  describe('when no records are provided', () => {
    beforeEach(() => {
      ({ container } = render(
        <MemoryRouter>
          <DomainSecurityDetailsMx domainId="123" type="SPF">
            <p>bleh</p>
          </DomainSecurityDetailsMx>
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
