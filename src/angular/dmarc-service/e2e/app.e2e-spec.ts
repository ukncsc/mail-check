import { DmarcServicePage } from './app.po';

describe('dmarc-service App', function() {
  let page: DmarcServicePage;

  beforeEach(() => {
    page = new DmarcServicePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
