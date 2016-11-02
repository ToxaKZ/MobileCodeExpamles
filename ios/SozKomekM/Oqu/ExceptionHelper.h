//
//  ExceptionHelper.h
//  Oqu
//
//  Created by Ельнар Шопанов on 29.02.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface ExceptionHelper : NSObject

+ (ExceptionHelper*)sharedInstance;
- (void)alertWithException:(NSException*)e;

@end