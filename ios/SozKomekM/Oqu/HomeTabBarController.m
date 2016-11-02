//
//  HomeTabBarController.m
//  Oqu
//
//  Created by Ельнар Шопанов on 06.03.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import "HomeTabBarController.h"
#import "HistoryViewController.h"
#import "FavoritesViewController.h"

@implementation HomeTabBarController

- (void)setSelectedViewController:(__kindof UIViewController *)selectedViewController {
    [super setSelectedViewController:selectedViewController];
    
    @try {
        [self checkControllers:selectedViewController];
    }
    @catch (NSException *e) {
    }
}

- (BOOL)checkControllers:(__kindof UIViewController *)view {
    NSArray *childViewControllers = [view childViewControllers];
    
    if ([childViewControllers count] == 0) {
        return NO;
    }
    
    for (__kindof UIViewController *child in childViewControllers) {
        if (child == nil) {
            continue;
        }
        
        if ([child isKindOfClass:[HistoryViewController class]]) {
            [((HistoryViewController*)child) refresh];
            return YES;
        } else if ([child isKindOfClass:[FavoritesViewController class]]) {
            [((FavoritesViewController*)child) refresh];
            return YES;
        }
        
        if ([self checkControllers:child]) {
            return YES;
        }
    }
    
    return NO;
}

@end