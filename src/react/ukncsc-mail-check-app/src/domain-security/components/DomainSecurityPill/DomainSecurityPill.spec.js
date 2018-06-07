import React from 'react';
import { render } from 'react-testing-library';
import DomainSecurityPill from './DomainSecurityPill';

describe('DomainSecurityPill', () => {
  let container;

  describe('when getting an error pill', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecurityPill error>1 error!</DomainSecurityPill>
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });

  describe('when getting a warning pill', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecurityPill warning>1 warning!</DomainSecurityPill>
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });

  describe('when getting an information pill', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecurityPill error>1 info!</DomainSecurityPill>
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });

  describe('when getting a success pill', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecurityPill>1 success!</DomainSecurityPill>
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });
});
