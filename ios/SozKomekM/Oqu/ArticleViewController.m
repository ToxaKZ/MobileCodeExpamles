//
//  ArticleViewController.m
//  Oqu
//
//  Created by Ельнар Шопанов on 28.02.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import "ArticleViewController.h"
#import "DatabaseHelper.h"
#import "ArticleModel.h"
#import "ExceptionHelper.h"
#import "HistoryModel.h"
#import "FavoriteModel.h"
#import "Constants.h"

@implementation ArticleViewController

@synthesize webView;

+ (void)showWithParent:(UIViewController*)parent wordID:(NSInteger)wordId wordName:(NSString*)wordName history:(BOOL)history {
    NSArray *articles = [[DatabaseHelper instance] articlesWithWordId:wordId];
    
    if (articles == nil || articles.count == 0) {
        @throw [NSException exceptionWithName:@"WordNotFound" reason:NSLocalizedString(KEY_COULD_NOT_FIND_TRANSLATION, nil) userInfo:nil];
    }
    
    if (history) {
        [[DatabaseHelper instance] insertHistory:[HistoryModel historyWithWordId:wordId name:wordName]];
    }
    
    ArticleViewController *controller = [parent.storyboard instantiateViewControllerWithIdentifier:@"ArticleViewController"];
    [controller initializeWithWordID:wordId wordName:wordName history:history articles:articles];
    [parent.navigationController pushViewController:controller animated:YES];
}

- (void)viewDidLoad {
    [super viewDidLoad];
    
    _styleCSSPath = [[NSBundle mainBundle] pathForResource:@"style" ofType:@"css"];
    
//    for (id subview in webView.subviews) {
//        if ([[subview class] isSubclassOfClass: [UIScrollView class]]) {
//            ((UIScrollView *)subview).bounces = NO;
//        }
//    }
    
    [self setupLanguageTabs];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
}

- (void)initializeWithWordID:(NSInteger)wordId wordName:(NSString*)wordName history:(BOOL)history articles:(NSArray*)articles {
    _wordId = wordId;
    _wordName = wordName;
    _history = history;
    _articles = articles;
}

- (void)setupLanguageTabs {
    _segmentedControl = nil;
    _cultures = [NSMutableArray array];
    
    for (ArticleModel *item in _articles) {
        if (![_cultures containsObject:item.toCulture]) {
            [_cultures addObject:item.toCulture];
        }
    }
    
    if (_cultures.count > 1) {
        _segmentedControl = [[UISegmentedControl alloc] initWithFrame:CGRectMake(0, 0, 130, 30)];
        [_segmentedControl addTarget:self action:@selector(segmentControlChangeValue:) forControlEvents:UIControlEventValueChanged];
        
        for (NSString *child in _cultures) {
            [_segmentedControl insertSegmentWithTitle:child atIndex:_segmentedControl.numberOfSegments animated:NO];
        }
        
        if (_segmentedControl.numberOfSegments > 0) {
            _segmentedControl.selectedSegmentIndex = 0;
        }
        
        self.navigationItem.titleView = _segmentedControl;
    }
    
    [self setupWebView];
}

- (void)segmentControlChangeValue:(id)sender {
    @try {
        [self setupWebView];
    }
    @catch (NSException *e) {
        [[ExceptionHelper sharedInstance] alertWithException:e];
    }
}

- (IBAction)favoriteButtonClick:(id)sender {
    @try {
        [[DatabaseHelper instance] insertFavorite:[FavoriteModel favoriteWithWordId:_wordId name:_wordName]];
    }
    @catch (NSException *e) {
        [[ExceptionHelper sharedInstance] alertWithException:e];
    }
}

//- (NSString*)segmentTitleWithCulture:(NSString*)culture {
//    if ([@"kk" caseInsensitiveCompare:culture] == NSOrderedSame) {
//        return NSLocalizedString(@"kazakh", nil);
//    } else if ([@"ru" caseInsensitiveCompare:culture] == NSOrderedSame) {
//        return NSLocalizedString(@"russian", nil);
//    } else if ([@"en" caseInsensitiveCompare:culture] == NSOrderedSame) {
//        return NSLocalizedString(@"english", nil);
//    } else {
//        return nil;
//    }
//}

- (void)setupWebView {
    NSMutableString *body = [NSMutableString string];
    
    if (_articles != nil) {
        NSString *culture = [_cultures objectAtIndex:_segmentedControl != nil ? _segmentedControl.selectedSegmentIndex : 0];
        
        for (ArticleModel *item in _articles) {
            if ([item.toCulture caseInsensitiveCompare:culture] != NSOrderedSame) {
                continue;
            }
            
            if (body.length > 0) {
                [body appendString:@"<br>"];
            }
            
            NSString *dictionary = [NSString stringWithFormat:@"%@ (%@-%@)", item.dicName, item.fromCulture, item.toCulture];
            [body appendString:[item.name stringByReplacingOccurrencesOfString:@"-=d=-" withString:dictionary]];
        }
    }
    
    [body insertString:@"<!DOCTYPE HTML><html><head><meta charset=\"utf-8\"><link rel=\"stylesheet\" href=\"style.css\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1, minimum-scale=1, maximum-scale=1, user-scalable=0\"></head><body>" atIndex:0];
    [body appendString:@"</body></html>"];
    
    [webView loadHTMLString:body baseURL:[NSURL fileURLWithPath:_styleCSSPath]];
}

- (BOOL)webView:(UIWebView *)webView shouldStartLoadWithRequest:(NSURLRequest *)request navigationType:(UIWebViewNavigationType)navigationType {
    @try {
        if (navigationType == UIWebViewNavigationTypeLinkClicked) {
            NSArray *pathComponents = [_styleCSSPath pathComponents];
            NSInteger count = pathComponents.count;
            
            if (count > 0) {
                NSString *directoryPath = [NSString pathWithComponents:[pathComponents subarrayWithRange:NSMakeRange(0, count - 1)]];
                NSString *decodedUrl = [[request.URL absoluteString] stringByReplacingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
                NSString *lexeme = [[decodedUrl stringByReplacingOccurrencesOfString:directoryPath withString:@""] stringByReplacingOccurrencesOfString:@"file:///" withString:@""];
                NSString *text = [[DatabaseHelper instance] prepareTextForSearch:lexeme];
                
                if (text != nil && text.length > 0) {
                    int wordId = [[DatabaseHelper instance] wordIdWithText:text];
                    
                    if (wordId > 0) {
                        [ArticleViewController showWithParent:self wordID:wordId wordName:lexeme history:false];
                    }
                }
            }
            
            return NO;
        }
        
        return YES;
    }
    @catch (NSException *e) {
        [[ExceptionHelper sharedInstance] alertWithException:e];
    }
}

@end