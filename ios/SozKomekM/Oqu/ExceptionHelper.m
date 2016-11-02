//
//  ExceptionHelper.m
//  Oqu
//
//  Created by Ельнар Шопанов on 29.02.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import "ExceptionHelper.h"

@implementation ExceptionHelper

static ExceptionHelper *helper = nil;

+ (ExceptionHelper*)sharedInstance {
    if (helper == nil) {
        helper = [[ExceptionHelper alloc] init];
    }
    
    return helper;
}

- (void)alertWithException:(NSException*)e {
//    UIAlertView *alert = [[UIAlertView alloc] initWithTitle:nil message:e.reason delegate:nil cancelButtonTitle:NSLocalizedString(RES_KEY_NO, nil) otherButtonTitles:nil, nil];
//    [alert show];
}

@end