/* eslint-disable import/first */
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

  describe('when children are present', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecurityTitle title="a">
          <p>Last checked one day ago</p>
        </DomainSecurityTitle>
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('it should display render the children', () =>
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

    test('it should not have any icons', () =>
      expect(container.getElementsByTagName('i')).toHaveLength(0));
  });
});
