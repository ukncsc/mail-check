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
        <DomainSecurityTitle failures={['err!']}>a</DomainSecurityTitle>
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
        <DomainSecurityTitle lastChecked="2018-01-01T12:00">
          a
        </DomainSecurityTitle>
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
        <DomainSecurityTitle warnings={['warn!']}>a</DomainSecurityTitle>
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
        <DomainSecurityTitle inconclusives={['inconclusive!']}>
          a
        </DomainSecurityTitle>
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
      ({ container } = render(<DomainSecurityTitle>a</DomainSecurityTitle>));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('it should not have any icons', () =>
      expect(container.getElementsByTagName('i')).toHaveLength(0));
  });
});
