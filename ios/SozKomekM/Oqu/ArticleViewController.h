//
//  ArticleViewController.h
//  Oqu
//
//  Created by Ельнар Шопанов on 28.02.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface ArticleViewController : UIViewController <UIWebViewDelegate> {
    NSInteger _wordId;
    NSString *_wordName;
    BOOL _history;
    NSArray *_articles;
    UISegmentedControl *_segmentedControl;
    NSString *_styleCSSPath;
    NSMutableArray *_cultures;
}

@property (weak, nonatomic) IBOutlet UIWebView *webView;

- (IBAction)segmentControlChangeValue:(id)sender;

+ (void)showWithParent:(UIViewController*)parent wordID:(NSInteger)wordId wordName:(NSString*)wordName history:(BOOL)history;

@end