//
//  HistoryModel.h
//  Oqu
//
//  Created by Ельнар Шопанов on 03.03.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface HistoryModel : NSObject <NSCoding> {
    NSString *_name;
    NSInteger _wordId;
}

@property (nonatomic, copy, nonnull) NSString *name;
@property (nonatomic, assign) NSInteger wordId;

+ (nonnull HistoryModel*)historyWithWordId:(NSInteger)wordId name:(nonnull NSString*)name;

@end