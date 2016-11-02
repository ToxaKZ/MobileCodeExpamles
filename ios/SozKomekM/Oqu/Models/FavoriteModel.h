//
//  FavoriteModel.h
//  Oqu
//
//  Created by Ельнар Шопанов on 07.03.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface FavoriteModel : NSObject <NSCoding> {
    NSString *_name;
    NSInteger _wordId;
}

@property (nonatomic, copy, nonnull) NSString *name;
@property (nonatomic, assign) NSInteger wordId;

+ (nonnull id)favoriteWithWordId:(NSInteger)wordId name:(nonnull NSString*)name;

@end