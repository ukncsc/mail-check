/* eslint-disable import/first */
jest.mock('moment', () => () => ({ fromNow: () => 'one day ago' }));

import React from 'react';
import { render } from 'react-testing-library';
import 'dom-testing-library/extend-expect';
import DomainSecurityTitle from './DomainSecurityTitle';

describe('DomainSecurityTitle', () => {
  let container;

  describe('when failures are present', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecurityTitle title="a" failures={['err!']} />
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('it should have a red exclamation circle', () =>
      expect(
        container.getElementsByClassName('red exclamation circle icon')
      ).toHaveLength(1));
  });

  describe('when last checked is present', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecurityTitle title="a" lastChecked="2018-01-01T12:00" />
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('it should display last checked', () =>
      expect(container.getElementsByTagName('p')[0]).toHaveTextContent(
        'Last checked one day ago'
      ));
  });

  describe('when warnings are present without failures', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecurityTitle title="a" warnings={['warn!']} />
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('it should have a yellow exclamation triangle', () =>
      expect(
        container.getElementsByClassName('yellow exclamation triangle icon')
      ).toHaveLength(1));
  });

  describe('when inconclusives are present', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecurityTitle title="a" inconclusives={['inconclusive!']} />
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('it should have a grey question circle', () =>
      expect(
        container.getElementsByClassName('grey question circle icon')
      ).toHaveLength(1));
  });

  describe('when no warnings, failures or inconclusives are present', () => {
    beforeEach(() => {
      ({ container } = render(<DomainSecurityTitle title="a" />));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('it should have a green check circle', () =>
      expect(
        container.getElementsByClassName('green check circle icon')
      ).toHaveLength(1));
  });
});
